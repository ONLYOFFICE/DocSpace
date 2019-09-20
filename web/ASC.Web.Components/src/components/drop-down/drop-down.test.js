import React from 'react';
import { mount, shallow } from 'enzyme';
import DropDown from '.';

const baseProps = {
  opened: false,
  isOpen: false,
  directionX: 'left',
  directionY: 'bottom',
  withArrow: false,
  manualWidth: '100%'
};

const baseChildren = (
  <div label='1'></ div>
);

describe('<DropDown />', () => {
  it('rendered without error', () => {
    const wrapper = mount(<DropDown {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('opened/isOpen', () => {
    const wrapper = mount(<DropDown {...baseProps} opened isOpen />);

    expect(wrapper.prop('opened')).toEqual(true);
    expect(wrapper.prop('isOpen')).toEqual(true);
  });

  it('directionX right', () => {
    const wrapper = mount(<DropDown {...baseProps} directionX='right' />);

    expect(wrapper.prop('directionX')).toEqual('right');
  });

  it('directionY top', () => {
    const wrapper = mount(<DropDown {...baseProps} directionY='top' />);

    expect(wrapper.prop('directionY')).toEqual('top');
  });

  it('withArrow', () => {
    const wrapper = mount(<DropDown {...baseProps} withArrow />);

    expect(wrapper.prop('withArrow')).toEqual(true);
  });

  it('manualY', () => {
    const wrapper = mount(<DropDown {...baseProps} manualY='100%' />);

    expect(wrapper.prop('manualY')).toEqual('100%');
  });

  it('manualX', () => {
    const wrapper = mount(<DropDown {...baseProps} manualX='100%' />);

    expect(wrapper.prop('manualX')).toEqual('100%');
  });

  it('isUserPreview', () => {
    const wrapper = mount(<DropDown {...baseProps} isUserPreview />);

    expect(wrapper.prop('isUserPreview')).toEqual(true);
  });

  it('with children', () => {
    const wrapper = mount(
      <DropDown {...baseProps}>
        {baseChildren}
      </DropDown>
    );

    expect(wrapper.children()).toHaveLength(1);
  });
  /*
  it('with maxHeight and children', () => {
    const wrapper = mount((
      <DropDown {...baseProps} maxHeight={200}>
        <div>1</div>
      </DropDown>
    ));

    expect(wrapper.children()).toHaveLength(1);
  });
  */
});
