import React from 'react';
import { mount } from 'enzyme';
import EmptyScreenContainer from '.';

describe('<EmptyScreenContainer />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <EmptyScreenContainer
        imageSrc="empty_screen_filter.png"
        imageAlt="Empty Screen Filter image"
        headerText="No results matching your search could be found"
        descriptionText="No results matching your search could be found"
        buttons={
          <a href="/">Go to home</a>
        }
      />
    );

    expect(wrapper).toExist();
  });
});
