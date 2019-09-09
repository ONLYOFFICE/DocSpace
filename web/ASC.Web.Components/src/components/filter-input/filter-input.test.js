import React from 'react';
import { mount } from 'enzyme';
import FilterInput from '.';

describe('<FilterInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <FilterInput
        getFilterData={() => [{ key: 'filter-example', group: 'filter-example', label: 'example group', isHeader: true }, { key: '0', group: 'filter-example', label: 'Test' }]}
        getSortData={() => [{ key: 'name', label: 'Name' }, { key: 'surname', label: 'Surname' }]}
        onFilter={(result) => { console.log(result) }}
      />
    );

    expect(wrapper).toExist();
  });
});
