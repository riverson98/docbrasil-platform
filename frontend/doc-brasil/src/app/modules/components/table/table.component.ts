import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { StatusPipe } from "../../../core/pipes/status.pipe";
import { FunctionPipe } from "../../../core/pipes/function.pipe";
import { getStatusClass } from '../../../script/_global-scripts';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    StatusPipe, 
    FunctionPipe,
    CommonModule
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent {
  @Input({alias: 'data', required: true}) data:any[] = [];
  @Output() requestDataUpdate = new EventEmitter<void>();
  @Input({alias: 'currentPage', required: true}) currentPage!: number;
  @Input({alias: 'totalPages', required: true}) totalPages!: number;
  @Output() pageChange = new EventEmitter<number>();

  ngOnChanges(changes: SimpleChanges) {
    if (changes['data']) {
      console.log('Dados atualizados recebidos do pai:', this.data);
    }
  }

  onOpenModal() {
    this.requestDataUpdate.emit();
  }
  
  getStatus(status: number): string { 
    return getStatusClass(status)
  }

  goToPrevious() {
    if (this.currentPage > 1) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }
  
  goToNext() {
    if (this.currentPage < this.totalPages) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }
}
