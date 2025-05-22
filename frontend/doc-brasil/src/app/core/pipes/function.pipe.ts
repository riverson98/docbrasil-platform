import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'functionPipe',
  standalone: true
})
export class FunctionPipe implements PipeTransform {

  transform(value: number): string {
    switch(value){
      case 0:
        return 'Associado';
      case 1:
        return 'Representante';
      case 2:
        return 'Administrador';
      case 3:
        return 'Diretor';
      default:
        return 'Desconhecido';
    }
  }
}
