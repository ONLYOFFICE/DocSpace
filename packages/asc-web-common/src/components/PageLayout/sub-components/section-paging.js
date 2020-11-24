import React from "react";
import styled from "styled-components";
import isEqual from "lodash/isEqual";

const StyledSectionPaging = styled.div`
  margin: 16px 0 0;
`;

class SectionPaging extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    return <StyledSectionPaging {...this.props} />;
  }
}

SectionPaging.displayName = "SectionPaging";

export default SectionPaging;
