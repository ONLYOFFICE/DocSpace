import React from 'react';
import { mount } from 'enzyme';

import ModuleTile from './';

describe('<ModuleTile />', () => {
  it('renders without error', () => {
    const wrapper = mount(<ModuleTile
        title="Documents"
        imageUrl="./modules/documents240.png"
        link="/products/files/"
        description="Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed."
        isPrimary={true}
        />
    );

    expect(wrapper).toExist();
  });
});
