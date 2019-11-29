import React from 'react';
import { mount, shallow } from 'enzyme';
import SocialButtonWithToggle from '.';

describe('<SocialButtonWithToggle />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <SocialButtonWithToggle iconName={'ShareFacebookIcon'} label={"Test Caption"}/>
    );

    expect(wrapper).toExist();
  });

  it('disabled click test', () => {
    const testClick = jest.fn();

    const wrapper = mount(<SocialButtonWithToggle iconName={'ShareFacebookIcon'} label={"Test Caption"} onClick={testClick} isDisabled={true}/>);

    wrapper.find('button.button').simulate('click');
    expect(testClick).toHaveBeenCalledTimes(0);   
    wrapper.find('button.toggle').simulate('click');
    expect(testClick).toHaveBeenCalledTimes(0);   
  });

  it('click test', () => {
    const testClick = jest.fn();

    const wrapper = mount(<SocialButtonWithToggle iconName={'ShareFacebookIcon'} label={"Test Caption"} onClick={testClick} isDisabled={false}/>);

    wrapper.find('button.button').simulate('click');
    expect(testClick).toHaveBeenCalledTimes(1);   
    wrapper.find('button.toggle').simulate('click');
    expect(testClick).toHaveBeenCalledTimes(1);   
  });
});
