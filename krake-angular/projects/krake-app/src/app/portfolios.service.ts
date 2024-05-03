import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, catchError, of, retry, tap, throwError } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class PortfolioService {
    private readonly http = inject(HttpClient);
    title = "krake-app";
    portfolios: PortfolioResponse[] = [];

    constructor() {}

    listPortfolios(): Observable<PortfolioResponse[]> {
        return this.portfolios.length
            ? of(this.portfolios)
            : this.http
                  .get<PortfolioResponse[]>("/api/portfolios", {
                      responseType: "json"
                  })
                  .pipe(
                      tap((portfolios: PortfolioResponse[]) => (this.portfolios = portfolios)),
                      tap(() => console.log("Set Portfolios: ", this.portfolios)),
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

export interface PortfolioResponse {
    id: string;
    name: string;
}
