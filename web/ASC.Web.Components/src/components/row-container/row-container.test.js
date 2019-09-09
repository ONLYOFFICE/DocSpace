import React from 'react';
import { mount } from 'enzyme';
import Row from '.';

describe('<Row />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Row 
          checked={false}
          contextOptions={[]}
      >
          <span>Some text</span>
      </Row>
    );

    expect(wrapper).toExist();
  });
});
