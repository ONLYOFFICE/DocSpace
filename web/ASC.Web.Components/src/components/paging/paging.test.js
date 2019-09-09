import React from 'react';
import { mount } from 'enzyme';
import Paging from '.';

describe('<Paging />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Paging 
          previousLabel='Previous'
          nextLabel='Next'
          selectedPageItem={{ label: "1 of 1"}}
          selectedCountItem={{ label: "25 per page"}}
          previousAction={() => console.log('Prev')}
          nextAction={() => console.log('Next')}
          openDirection='bottom'
          onSelectPage={(a) => console.log(a)}
          onSelectCount={(a) => console.log(a)}
      />
    );

    expect(wrapper).toExist();
  });
});
