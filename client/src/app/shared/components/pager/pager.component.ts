import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-pager',
  templateUrl: './pager.component.html',
  styleUrls: ['./pager.component.scss'],
})
export class PagerComponent implements OnInit {
  @Input() pageSize: number;
  @Input() totalCount: number;
  @Input() pageNumber: number;

  @Output() pageChanged = new EventEmitter<number>();

  constructor() {}

  ngOnInit(): void {}

  onPagerChanged(event: any): void {
    this.pageChanged.emit(event.page);
  }
}
