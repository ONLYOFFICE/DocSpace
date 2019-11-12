import React from 'react';
import { mount, shallow } from 'enzyme';
import RowContainer from '.';

const baseProps = {
  manualHeight: '500px'
};

describe('<RowContainer />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <RowContainer {...baseProps}>
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper).toExist();
  });

  it('renders without manualHeight', () => {
    const wrapper = mount(
      <RowContainer>
        <span>Demo</span>
      </RowContainer>
    );

    expect(wrapper).toExist();
  });

  it('call onRowContextClick() with normal options', () => {
    const options = [
      {
        key: '1',
        label: 'test'
      }
    ];

    const wrapper = mount(
      <RowContainer>
        <span>Demo</span>
      </RowContainer>
    );

    const instance = wrapper.instance();

    instance.onRowContextClick(options);

    expect(wrapper.state('contextOptions')).toEqual(options);
  });

  it('call onRowContextClick() with wrong options', () => {
    const options =
    {
      key: '1',
      label: 'test'
    }
      ;

    const wrapper = mount(
      <RowContainer>
        <span>Demo</span>
      </RowContainer>
    );

    const instance = wrapper.instance();

    instance.onRowContextClick(options);

    expect(wrapper.state('contextOptions')).toEqual([]);
  });

  it('componentWillUnmount() props lifecycle test', () => {
    const wrapper = shallow(
      <RowContainer>
        <span>Demo</span>
      </RowContainer>);
    const instance = wrapper.instance();

    instance.componentWillUnmount();

    expect(wrapper).toExist(false);
  });

  it('render with normal rows', () => {
    const wrapper = mount(
      <RowContainer {...baseProps}>
        <div contextOptions={[{key: '1', label: 'test'}]}>test</div>
      </RowContainer>
    );

    expect(wrapper).toExist();
  });
});
