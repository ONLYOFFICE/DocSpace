import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { utils, Scrollbar, DragAndDrop, Box } from "asc-web-components";
import SelectedFrame from "./SelectedFrame";
import isEqual from "lodash/isEqual";
import { isMobile } from "react-device-detect";
import {RefContextConsumer} from "asc-web-common"

const { tablet, smallTablet } = utils.device;

const commonStyles = css`
  flex-grow: 1;
  height: 100%;

.wrapper-container{
  position: relative;
    width: 100%;
    height: 100%; 
    z-index:0;

}
.inner-container{
 position:absolute; 
  width:100%; 
}
  @media ${tablet} {
      //width:100%;
    }

  .section-wrapper-content {
    flex: 1 0 auto;
    padding: 16px 8px 16px 24px;
    outline: none;
    ${(props) => props.viewAs == "tile" && "padding-right:0;"}

    @media ${tablet} {
      padding: 16px 0 16px 24px;
      margin-top: 59px; 
      
    }
    @media ${smallTablet} {
     
      margin-top: 79px; 
      
    }
    .section-wrapper {
      display: flex;
      flex-direction: column;
      min-height: 100%;

    }
  }
`;

const StyledSectionBody = styled.div`
  ${commonStyles}

  ${(props) =>
    props.withScroll &&
    `
    margin-left: -24px;
  `}
`;

const StyledDropZoneBody = styled(DragAndDrop)`
  ${commonStyles}

  .drag-and-drop {
    user-select: none;
    height: 100%;
    
  }

  ${(props) =>
    props.withScroll &&
    `
    margin-left: -24px;
  `}
`;

const StyledSpacer = styled.div`
  display: none;
  min-height: 64px;

  @media ${tablet} {
    display: ${(props) => (props.pinned ? "none" : "block")};
  }
`;

class SectionBody extends React.Component {
  constructor(props) {
    super(props);

    this.focusRef = React.createRef();
    this.scrollRef = React.createRef();
    this.windowWidth = window.matchMedia( "(max-width: 1024px)" );
  }

  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  componentDidMount() {
    if (!this.props.autoFocus) return;

    this.focusRef.current.focus();
  }

  render() {

    //console.log("PageLayout SectionBody render");
    const {
      autoFocus,
      children,
      onDrop,
      pinned,
      setSelections,
      uploadFiles,
      viewAs,
      withScroll,
     
      widthProp
    } = this.props;

    const focusProps = autoFocus
      ? {
        ref: this.focusRef,
        tabIndex: 1,
      }
      : {};

    const scrollProp = uploadFiles ? { ref: this.scrollRef } : {};

    return uploadFiles ? (

      <StyledDropZoneBody
        isDropZone
        onDrop={onDrop}
        withScroll={withScroll}
        viewAs={viewAs}
      >
   
        {withScroll ? (
        
          (!isMobile && !this.windowWidth.matches ) ? (
            <Scrollbar {...scrollProp} stype="mediumBlack" >
              <SelectedFrame
                viewAs={viewAs}
                scrollRef={this.scrollRef}
                setSelections={setSelections}
              >
                <div className="section-wrapper">
                  <div className="section-wrapper-content" {...focusProps}>
                    {children}
                    <StyledSpacer pinned={pinned} />
                  </div>
                </div>
              </SelectedFrame>
            </Scrollbar>)
            : (
              
              <Box className="wrapper-container">
              <Box  className="inner-container">
              <RefContextConsumer>
                {value => 
                  <SelectedFrame
                    viewAs={viewAs}
                    scrollRef={value}
                    setSelections={setSelections}
                  > 
                        <div className="section-wrapper">
                          <div className="section-wrapper-content" {...focusProps}>
                            {children}
                            <StyledSpacer pinned={pinned} />
                          </div>
                        </div>
                        
      
                  </SelectedFrame>
                  }
              </RefContextConsumer>
              </Box>
              </Box>)
        ) : (
            <SelectedFrame
              viewAs={viewAs}
              scrollRef={this.scrollRef}
              setSelections={setSelections}
            >
              <div className="section-wrapper">
                {children}
                <StyledSpacer pinned={pinned} />

              </div>
            </SelectedFrame>
          )}
      </StyledDropZoneBody>
    ) : (
        <StyledSectionBody viewAs={viewAs} withScroll={withScroll}>
          {withScroll ? (
               (!isMobile && !this.windowWidth.matches )  ? (
              <Scrollbar {...scrollProp} stype="mediumBlack">
                <div className="section-wrapper">
                  <div className="section-wrapper-content" {...focusProps}>
                    {children}
                    <StyledSpacer pinned={pinned} />
                  </div>
                </div>
              </Scrollbar>)
              : (
                <Box className="wrapper-container">
                <Box  className="inner-container">
                <div className="section-wrapper">
                  <div className="section-wrapper-content" {...focusProps}>
                    {children}
                    <StyledSpacer pinned={pinned} />
                  </div>
                </div>
                </Box>
                </Box>
              )
          ) : (
              <div className="section-wrapper">
                {children}
                <StyledSpacer pinned={pinned} />
              </div>
            )}
        </StyledSectionBody>
      );
  }
}

SectionBody.displayName = "SectionBody";

SectionBody.propTypes = {
  withScroll: PropTypes.bool,
  autoFocus: PropTypes.bool,
  pinned: PropTypes.bool,
  onDrop: PropTypes.func,
  setSelections: PropTypes.func,
  uploadFiles: PropTypes.bool,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
    PropTypes.any,
  ]),
  viewAs: PropTypes.string,
};

SectionBody.defaultProps = {
  autoFocus: false,
  pinned: false,
  uploadFiles: false,
  withScroll: true,
};

export default SectionBody;
