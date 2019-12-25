import React from 'react';
import { mount } from 'enzyme';
import Layout from '.';

const baseProps = {
  isBackdropVisible: false,
  isNavHoverEnabled: true,
  isNavOpened: false,
  isAsideVisible: false,
  currentUser: null,
  currentUserActions: [],
  availableModules: []
}

// eslint-disable-next-line no-undef
const later = (delay) => new Promise(function (resolve) {
  setTimeout(resolve, delay);
});

describe('<Layout />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it('renders with modules', () => {
    const modules = [
      {
        id: 'test',
        separator: true
      },
      {
        id: 'demo',
        title: 'demo',
        iconName: 'CalendarCheckedIcon',
        notifications: 0
      },
      {
        id: 'demo1',
        title: 'demo1',
        iconName: 'CalendarCheckedIcon',
        notifications: 0,
        isolateMode: true
      }
    ];

    const wrapper = mount(
      <Layout availableModules={modules} currentModuleId='demo' />
    ).instance();

    expect(wrapper.props.availableModules).toBe(modules);
  });

  it("componentDidUpdate() test", () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    ).instance();

    wrapper.componentDidUpdate(wrapper.props);

    expect(wrapper.props).toBe(wrapper.props);

    wrapper.componentDidUpdate(
      {
        currentModuleId: 'demo',
        currentUser: {
          id: 'test',
          displayName: 'test test',
          email: 'test',
          avatarSmall: 'test'
        },
        availableModules: [
          {
            id: 'test',
            separator: true
          }
        ]
      });

    expect(wrapper.props).toBe(wrapper.props);
  });

  it("call backdropClick()", () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    ).instance();

    wrapper.backdropClick();

    expect(wrapper.state.isBackdropVisible).toBe(false);
    expect(wrapper.state.isNavOpened).toBe(false);
    expect(wrapper.state.isAsideVisible).toBe(false);
  });

  it("call showNav()", () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    ).instance();

    wrapper.showNav();

    expect(wrapper.state.isBackdropVisible).toBe(true);
    expect(wrapper.state.isNavOpened).toBe(true);
    expect(wrapper.state.isAsideVisible).toBe(false);
    expect(wrapper.state.isNavHoverEnabled).toBe(false);
  });

  it("call toggleAside()", () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    ).instance();

    wrapper.toggleAside();

    expect(wrapper.state.isBackdropVisible).toBe(true);
    expect(wrapper.state.isNavOpened).toBe(false);
    expect(wrapper.state.isAsideVisible).toBe(true);
    expect(wrapper.state.isNavHoverEnabled).toBe(false);
  });

  it("call handleNavMouseEnter()", async () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    ).instance();

    wrapper.setState({ isNavHoverEnabled: false });

    wrapper.handleNavMouseEnter();

    expect(wrapper.state.isNavOpened).toBe(false);

    wrapper.setState({ isNavHoverEnabled: true });

    wrapper.handleNavMouseEnter();

    await later(1100);

    expect(wrapper.state.isNavOpened).toBe(true);
  });

  it("call handleNavMouseLeave()", () => {
    const wrapper = mount(
      <Layout {...baseProps} />
    ).instance();

    wrapper.setState({ isNavHoverEnabled: false });

    wrapper.handleNavMouseLeave();

    expect(wrapper.state.isNavOpened).toBe(false);

    wrapper.setState({ isNavHoverEnabled: true });

    wrapper.timeout = 1;

    wrapper.handleNavMouseLeave();

    expect(wrapper.state.isNavOpened).toBe(false);
  });
});
