import { AsyncPipe, JsonPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
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
    portfolioService = inject(PortfolioService);
    portfolios$ = this.portfolioService.listPortfolios();
    title = "krake-app";
}
