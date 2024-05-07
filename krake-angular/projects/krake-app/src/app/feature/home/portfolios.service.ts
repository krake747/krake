import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, catchError, of, retry, tap, throwError } from "rxjs";

export interface Portfolio {
    id: string;
    name: string;
    currency: string;
    investments: PortfolioInvestment[];
}

export interface PortfolioInvestment {
    instrumentId: string;
    instrumentName: string;
    instrumentCurrency: string;
    purchaseDate: string;
    purchasePrice: number;
    quantity: number;
}

@Injectable({
    providedIn: "root"
})
export class PortfolioService {
    private readonly http = inject(HttpClient);
    #portfolios: Portfolio[] = [];
    constructor() {}

    listPortfoliosWithInvestments(): Observable<Portfolio[]> {
        return this.#portfolios.length
            ? of(this.#portfolios)
            : this.http.get<Portfolio[]>("/api/portfolios/investments").pipe(
                  tap(portfolios => (this.#portfolios = portfolios)),
                  retry({ count: 2, delay: 5000 }),
                  catchError(this.handleError)
              );
    }

    public handleError(err: HttpErrorResponse): Observable<never> {
        if (err.error instanceof ErrorEvent) {
            console.warn("Client", err.message); // client-side
        } else {
            console.warn("Server", err.status); // server-side
        }

        return throwError(() => new Error(err.message));
    }
}
