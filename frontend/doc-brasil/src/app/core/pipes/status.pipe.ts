import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'status',
  standalone: true
})
export class StatusPipe implements PipeTransform {

  transform(value: number): string {
    switch(value){
      case 0:
        return 'Pendente';
      case 1:
        return 'Ativo';
      case 2:
        return 'Inativo';
      default:
        return 'Desconhecido'
    }
  }

}
