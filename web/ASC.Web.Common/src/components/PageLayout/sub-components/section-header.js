import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
import isEqual from "lodash/isEqual";

const { tablet } = utils.device;

const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 55px;
  margin-right: 24px;
  margin-top: -1px;

  @media ${tablet} {
    margin-right: 16px;
    border-bottom: none;
    ${(props) =>
      props.borderBottom &&
      `
      border-bottom: 1px solid #eceef1;
      padding-bottom: 16px
    `};
    height: 49px;
  }

  .section-header {
    width: calc(100% - 76px);

    @media ${tablet} {
      width: 100%;
      padding-top: 4px;
    }

    h1,
    h2,
    h3,
    h4,
    h5,
    h6 {
      max-width: calc(100vw - 435px);

      @media ${tablet} {
        max-width: ${(props) =>
          props.isArticlePinned ? `calc(100vw - 320px)` : `calc(100vw - 96px)`};
      }
    }
  }
`;

class SectionHeader extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout SectionHeader render");
    // eslint-disable-next-line react/prop-types
    const { isArticlePinned, borderBottom, ...rest } = this.props;

    return (
      <StyledSectionHeader
        isArticlePinned={isArticlePinned}
        borderBottom={borderBottom}
      >
        <div className="section-header" {...rest} />
      </StyledSectionHeader>
    );
  }
}

SectionHeader.displayName = "SectionHeader";

export default SectionHeader;
