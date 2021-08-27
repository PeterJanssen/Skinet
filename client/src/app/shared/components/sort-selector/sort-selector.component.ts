import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IParam, ISortOption } from '../../models';

@Component({
  selector: 'app-sort-selector',
  templateUrl: './sort-selector.component.html',
  styleUrls: ['./sort-selector.component.scss'],
})
export class SortSelectorComponent implements OnInit {
  @Input() title: string;
  @Input() params: IParam;
  @Input() sortOptions: ISortOption[] = [];

  constructor() {}

  ngOnInit(): void {}

  onSortSelected(sort: string) {
    this.params.sort = sort;
  }
}
