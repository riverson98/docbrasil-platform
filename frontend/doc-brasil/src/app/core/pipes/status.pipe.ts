import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'status',
  standalone: true
})
export class StatusPipe implements PipeTransform {

  transform(value: number): string {
    switch(value){
      case 1:
        return 'Ativo';
      case 2:
        return 'Inativo';
      case 3:
        return 'Pendente';
      default:
        return 'Desconhecido'
    }
  }

}
