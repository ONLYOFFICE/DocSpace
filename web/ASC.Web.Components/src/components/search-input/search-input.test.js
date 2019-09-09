import React from 'react';
import { mount } from 'enzyme';
import SearchInput from '.';

describe('<SearchInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <SearchInput 
          isNeedFilter={true}
          getFilterData={() => [  { key: 'filter-example', group: 'filter-example', label: 'example group', isHeader: true },
                                  { key: 'filter-example-test', group: 'filter-example', label: 'Test' }]
                          }
          onSearchClick={(result) => {console.log(result)}}
          onChangeFilter={(result) => {console.log(result)}}
      />
    );

    expect(wrapper).toExist();
  });
});
