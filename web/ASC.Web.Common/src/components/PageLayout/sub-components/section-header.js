import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
import isEqual from "lodash/isEqual";
import classnames from "classnames";

const { tablet } = utils.device;

const StyledSectionHeader = styled.div`

 
  border-bottom: 1px solid #eceef1;
  height: 56px;
  margin-right: 24px;

  @media ${tablet} {
    margin-right: 16px;
    border-bottom: none;
    height: 44px;

 
  }
  .section-header--hidden {
    @media ${tablet} {
      top: -50px;
    }

}
  .section-header {
    width: calc(100% - 76px);

    @media ${tablet} {
      width: 100%;
      background-color: #fff; 
      position: fixed; 
      top: 56px; 
      padding-right:100%;
      transition: top 0.3s;
      z-index:1;
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
  constructor(props) {
    super(props);
    this.state = {
      prevScrollpos: window.pageYOffset || document.documentElement.scrollTop,
      visible: true
    };
    this.focusRef = React.createRef();
  }

  componentDidMount() {
 
    window.addEventListener("scroll", this.scrolledTheVerticalAxis);
  }


  componentWillUnmount() {
    window.removeEventListener("scroll", this.scrolledTheVerticalAxis);
  }

  scrolledTheVerticalAxis = () => {
    const { prevScrollpos } = this.state;

    const currentScrollPos = window.pageYOffset || document.documentElement.scrollTop;
    const visible = prevScrollpos > currentScrollPos;
    
  
    this.setState({
      prevScrollpos: currentScrollPos,
      visible
    });
  };

  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {

    //console.log("PageLayout SectionHeader render");
    // eslint-disable-next-line react/prop-types
    const { isArticlePinned, ...rest } = this.props;
    return (
      <StyledSectionHeader isArticlePinned={isArticlePinned}>
        <div id="scroll" className={classnames("section-header" , {
          "section-header--hidden": !this.state.visible
        })} {...rest} />
      </StyledSectionHeader>
    );
  }
}

SectionHeader.displayName = "SectionHeader";

export default SectionHeader;
