import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { EMPTY, Observable, Subject, forkJoin } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { ClimaService, ClimaAtualResponse, PrevisaoClimaResponse } from '../../core/services/services_clima';
import { AuthStateService } from '../../core/services/auth-state.service';

interface PrevisaoDiaria {
  data: Date;
  temperaturaMin: number;
  temperaturaMax: number;
  descricao: string;
  iconeTempo?: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnDestroy {
  textobusca = '';
  carregando = false;
  erro = '';
  climaAtual?: ClimaAtualResponse;
  previsao5Dias: PrevisaoDiaria[] = [];

  private readonly destruir$ = new Subject<void>();
  private readonly buscaCidade$ = new Subject<string>();

  constructor(
    private router: Router,
    private climaService: ClimaService,
    private authState: AuthStateService,
    private cdr: ChangeDetectorRef
  ) {
    this.buscaCidade$
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((valor) => this.consultarClima(valor)),
        takeUntil(this.destruir$)
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destruir$.next();
    this.destruir$.complete();
  }

  pesquisar(): void {
    this.buscaCidade$.next(this.textobusca);
  }

  onTextoBuscadoChange(valor: string): void {
    this.textobusca = valor;
    this.buscaCidade$.next(valor);
  }

  imagemIcone(icone?: string): string {
    return icone
      ? `https://openweathermap.org/img/wn/${icone}@2x.png` : 'https://openweathermap.org/img/wn/01d@2x.png';
  }

  labelTempo(descricao?: string): string {
    return descricao ? descricao : 'Condiçãoo não informada';
  }

  logout(): void {
    this.authState.clearToken();
    this.router.navigate(['/login']);
  }

  get autenticado(): boolean {
    return this.authState.isAuthenticated();
  }

  private consultarClima(valor: string): Observable<void> {
    const cidade = valor.trim();

    if (!cidade) {
      this.limparResultados();
      this.cdr.detectChanges();
      return EMPTY;
    }

    this.erro = '';
    this.carregando = true;
    this.climaAtual = undefined;
    this.previsao5Dias = [];
    this.cdr.detectChanges();

    return forkJoin({
      climaAtual: this.climaService.obterClimaAtual(cidade),
      previsao: this.climaService.obterPrevisao5Dias(cidade)
    }).pipe(
      map(({ climaAtual, previsao }) => {
        this.climaAtual = climaAtual;
        this.previsao5Dias = this.gerarPrevisaoDiaria(previsao);
        this.cdr.detectChanges();
      }),
      catchError((erro: unknown) => {
        this.erro = this.extrairMensagemErro(erro);
        this.cdr.detectChanges();
        return EMPTY;
      }),
      finalize(() => {
        this.carregando = false;
        this.cdr.detectChanges();
      })
    );
  }

  private limparResultados(): void {
    this.erro = '';
    this.carregando = false;
    this.climaAtual = undefined;
    this.previsao5Dias = [];
  }

  private gerarPrevisaoDiaria(dados: PrevisaoClimaResponse[]): PrevisaoDiaria[] {
    const porDia = new Map<string, PrevisaoDiaria>();

    for (const item of dados) {
      if (!item.dataPrevista) {
        continue;
      }

      const data = new Date(item.dataPrevista);
      const chave = data.toDateString();
      const temperaturaMin = item.temperaturaMin5dias ?? 0;
      const temperaturaMax = item.temperaturaMax5dias ?? 0;
      const existente = porDia.get(chave);

      if (!existente) {
        porDia.set(chave, {
          data,
          temperaturaMin,
          temperaturaMax,
          descricao: item.descricao ?? '',
          iconeTempo: item.iconeTempo
        });
        continue;
      }

      existente.temperaturaMin = Math.min(existente.temperaturaMin, temperaturaMin);
      existente.temperaturaMax = Math.max(existente.temperaturaMax, temperaturaMax);

      if (!existente.descricao && item.descricao) {
        existente.descricao = item.descricao;
      }

      if (!existente.iconeTempo && item.iconeTempo) {
        existente.iconeTempo = item.iconeTempo;
      }
    }

    return Array.from(porDia.values())
      .sort((a, b) => a.data.getTime() - b.data.getTime())
      .slice(0, 5);
  }

  private extrairMensagemErro(erro: unknown): string {
    if (erro instanceof HttpErrorResponse) {
      const detalhe = erro.error;
      return detalhe;
    }
    return 'Não foi possível consultar o clima. Tente novamente.';
  }
}
