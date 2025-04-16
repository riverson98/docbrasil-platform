import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-painel-title',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './painel-title.component.html',
  styleUrl: './painel-title.component.scss'
})
export class PainelTitleComponent implements OnInit{
  @Input({alias: 'title', required: true}) title:string = '';
  @Input({alias: 'description', required: true}) description:string = '';
  @Output() filterChanged = new EventEmitter<string>();
  filterControl = new FormControl('');

  ngOnInit() {
    this.filterControl.valueChanges
      .pipe(
        debounceTime(1000),
        distinctUntilChanged()
      )
      .subscribe(value => {
        const filter = value ?? '';
        this.filterChanged.emit(filter);
      });
  }
}
