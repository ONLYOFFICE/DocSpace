import React from 'react';
import { mount } from 'enzyme';
import TabContainer from '.';

describe('<TabContainer />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <TabContainer>{
        [
          {
              key: "0",
              title: "Title1",
              content:
                  <div >
                      <div> <button>BUTTON</button> </div>
                      <div> <button>BUTTON</button> </div>
                      <div> <button>BUTTON</button> </div>
                  </div>
          }
        ]
      }</TabContainer>
    );

    expect(wrapper).toExist();
  });
});
