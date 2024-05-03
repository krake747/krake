import { AsyncPipe, JsonPipe } from "@angular/common";
import { Component, inject, isDevMode } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { environment } from "../environments/environment.development";
import { PortfolioService } from "./portfolios.service";

@Component({
    selector: "krake-root",
    standalone: true,
    imports: [RouterOutlet, AsyncPipe, JsonPipe],
    template: `
        <h1>Welcome to {{ title }}!</h1>
        <pre>{{ portfolios$ | async | json }}</pre>
        <router-outlet />
    `,
    styles: []
})
export class AppComponent {
    portfolios$ = inject(PortfolioService).listPortfolios();
    title = "krake-app";

    constructor() {
        console.log(isDevMode() ? "Development!" : "Production!", `${environment.apiUrl}`);
    }
}
