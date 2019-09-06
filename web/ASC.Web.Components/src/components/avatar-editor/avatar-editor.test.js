import React from 'react';
import { mount } from 'enzyme';
import AvatarEditor from '.';

describe('<AvatarEditor />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <AvatarEditor
        visible={true}
        onSave={(data) =>{console.log(data.croppedImage, data.defaultImage)}}
      />
    );

    expect(wrapper).toExist();
  });
});
