import { TestBed } from '@angular/core/testing';
import { ResultDialog } from './result-dialog';

describe('ResultDialog', () => {
  let service: ResultDialog;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResultDialog);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
