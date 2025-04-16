import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PainelTitleComponent } from './painel-title.component';

describe('PainelTitleComponent', () => {
  let component: PainelTitleComponent;
  let fixture: ComponentFixture<PainelTitleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PainelTitleComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PainelTitleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
