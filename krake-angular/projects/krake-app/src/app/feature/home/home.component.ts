import { AsyncPipe, JsonPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { PortfolioResponse, PortfolioService } from "./portfolios.service";

import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { map } from "rxjs";

@Component({
    selector: "krake-home",
    standalone: true,
    imports: [AsyncPipe, JsonPipe, MatTableModule],
    template: `
        <div class="container">
            <h1>Portfolios</h1>
            @if (portfolios$ | async; as portfolios) {
                <table mat-table class="mat-elevation-z3" [dataSource]="portfolios">
                    <ng-container matColumnDef="id">
                        <th mat-header-cell *matHeaderCellDef>#</th>
                        <td mat-cell *matCellDef="let i = index">{{ i + 1 }}</td>
                    </ng-container>
                    <mat-text-column name="name"></mat-text-column>
                    <mat-text-column name="currency"></mat-text-column>
                    <tr mat-header-row *matHeaderRowDef="['id', 'name', 'currency']"></tr>
                    <tr mat-row *matRowDef="let row; columns: ['id', 'name', 'currency']"></tr>
                </table>
            }
        </div>
    `,
    styles: [
        `
            .container {
                padding: 15px;
            }
        `
    ]
})
export class HomeComponent {
    portfolios$ = inject(PortfolioService)
        .listPortfolios()
        .pipe(map(pfs => new MatTableDataSource<PortfolioResponse>(pfs)));
}
