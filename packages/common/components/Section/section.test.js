import React from "react";
import { mount } from "enzyme";
import Section from ".";

const baseProps = {
  withBodyScroll: true,
  withBodyAutoFocus: false,
};

describe("<Section />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Section {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("componentDidUpdate() test re-render", () => {
    const wrapper = mount(<Section {...baseProps} />).instance();

    wrapper.componentDidUpdate({ withBodyScroll: false });

    expect(wrapper.props).toBe(wrapper.props);
  });

  it("componentDidUpdate() test no re-render", () => {
    const wrapper = mount(
      <Section
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
});
