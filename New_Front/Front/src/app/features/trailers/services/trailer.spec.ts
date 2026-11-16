import { TestBed } from '@angular/core/testing';
import { Trailer } from './trailer';

describe('Trailer', () => {
  let service: Trailer;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Trailer);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
