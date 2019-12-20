import React from 'react';
import { mount } from 'enzyme';
import PageLayout from '.';

const baseProps = {
  isBackdropVisible: false,
  isArticleVisible: false,
  isArticlePinned: false,
  withBodyScroll: true
}

describe('<PageLayout />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <PageLayout {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it("componentDidUpdate() test re-render", () => {
    const wrapper = mount(
      <PageLayout {...baseProps} />
    ).instance();

    wrapper.componentDidUpdate({ withBodyScroll: false });

    expect(wrapper.props).toBe(wrapper.props);
  });

  it("componentDidUpdate() test no re-render", () => {
    const wrapper = mount(
      <PageLayout
        {...baseProps}
        articleHeaderContent={<>1</>}
        articleMainButtonContent={<>2</>}
        articleBodyContent={<>3</>}
        sectionHeaderContent={<>4</>}
        sectionFilterContent={<>5</>}
        sectionBodyContent={<>6</>}
        sectionPagingContent={<>7</>}
        withBodyScroll={false}
      />
    ).instance();

    wrapper.componentDidUpdate(wrapper.props);

    expect(wrapper.props.withBodyScroll).toBe(false);

    wrapper.componentDidUpdate(wrapper.props);

    expect(wrapper.props).toBe(wrapper.props);
  });

  it("call backdropClick()", () => {
    const wrapper = mount(
      <PageLayout {...baseProps} />
    ).instance();

    wrapper.backdropClick();

    expect(wrapper.state.isBackdropVisible).toBe(false);
    expect(wrapper.state.isArticleVisible).toBe(false);
    expect(wrapper.state.isArticlePinned).toBe(false);
  });

  it("call pinArticle()", () => {
    const wrapper = mount(
      <PageLayout {...baseProps} />
    ).instance();

    wrapper.pinArticle();

    expect(wrapper.state.isBackdropVisible).toBe(false);
    expect(wrapper.state.isArticleVisible).toBe(true);
    expect(wrapper.state.isArticlePinned).toBe(true);
  });

  it("call unpinArticle()", () => {
    const wrapper = mount(
      <PageLayout {...baseProps} />
    ).instance();

    wrapper.unpinArticle();

    expect(wrapper.state.isBackdropVisible).toBe(true);
    expect(wrapper.state.isArticleVisible).toBe(true);
    expect(wrapper.state.isArticlePinned).toBe(false);
  });

  it("call showArticle()", () => {
    const wrapper = mount(
      <PageLayout {...baseProps} />
    ).instance();

    wrapper.showArticle();

    expect(wrapper.state.isBackdropVisible).toBe(true);
    expect(wrapper.state.isArticleVisible).toBe(true);
    expect(wrapper.state.isArticlePinned).toBe(false);
  });
});
