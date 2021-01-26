import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { utils, Scrollbar, DragAndDrop } from "asc-web-components";
import SelectedFrame from "./SelectedFrame";
import equal from "fast-deep-equal/react";
import { LayoutContextConsumer } from "../../Layout/context";
import { getIsLoaded, getIsTabletView } from "../../../store/auth/selectors";
import { connect } from "react-redux";
import { isMobile } from "react-device-detect";

const { tablet } = utils.device;

const commonStyles = css`
  flex-grow: 1;
  height: 100%;

  .section-wrapper-content {
    flex: 1 0 auto;
    padding: 17px 7px 16px 24px;
    outline: none;
    ${(props) => props.viewAs == "tile" && "padding-right:0;"}

    @media ${tablet} {
      padding: 16px 0 16px 24px;
      /* ${(props) =>
        props.isLoaded &&
        isMobile &&
        css`
          margin-top: -7px;
        `} */
    }

    .section-wrapper {
      display: flex;
      flex-direction: column;
      min-height: 100%;
    }

    .people-row-container,
    .files-row-container {
      margin-top: -22px;
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
  }

  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  componentDidMount() {
    this.customScrollElm = document.querySelector(
      "#desktopScroll > .scroll-body"
    );
    if (!this.props.autoFocus) return;
    this.focusRef.current.focus();
  }

  componentWillUnmount() {
    this.focusRef = null;
    this.scrollRef = null;
  }

  render() {
    //console.log("PageLayout SectionBody render" );
    const {
      autoFocus,
      children,
      onDrop,
      pinned,
      setSelections,
      uploadFiles,
      viewAs,
      withScroll,
      isLoaded,
      isTabletView,
    } = this.props;

    const focusProps = autoFocus
      ? {
          ref: this.focusRef,
          tabIndex: 1,
        }
      : {};

    const scrollProp = uploadFiles ? { ref: this.scrollRef } : {};
    console.log("SectionBody", this.props);
    return uploadFiles ? (
      <StyledDropZoneBody
        isDropZone
        onDrop={onDrop}
        withScroll={withScroll}
        viewAs={viewAs}
        pinned={pinned}
        isLoaded={isLoaded}
      >
        {withScroll ? (
          !isMobile ? (
            <Scrollbar {...scrollProp} stype="mediumBlack">
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
            </Scrollbar>
          ) : (
            <LayoutContextConsumer>
              {(ref) => (
                <SelectedFrame
                  viewAs={viewAs}
                  scrollRef={ref.scrollRefLayout}
                  setSelections={setSelections}
                >
                  <div className="section-wrapper">
                    <div className="section-wrapper-content" {...focusProps}>
                      {children}
                      <StyledSpacer pinned={pinned} />
                    </div>
                  </div>
                </SelectedFrame>
              )}
            </LayoutContextConsumer>
          )
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
      <StyledSectionBody
        viewAs={viewAs}
        withScroll={withScroll}
        pinned={pinned}
        isLoaded={isLoaded}
      >
        {withScroll ? (
          !isMobile ? (
            <Scrollbar {...scrollProp} stype="mediumBlack">
              <div className="section-wrapper">
                <div className="section-wrapper-content" {...focusProps}>
                  {children}
                  <StyledSpacer pinned={pinned} />
                </div>
              </div>
            </Scrollbar>
          ) : (
            <div className="section-wrapper">
              <div className="section-wrapper-content" {...focusProps}>
                {children}
                <StyledSpacer pinned={pinned} />
              </div>
            </div>
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
  isLoaded: PropTypes.bool,
  isTabletView: PropTypes.bool,
};

SectionBody.defaultProps = {
  autoFocus: false,
  pinned: false,
  uploadFiles: false,
  withScroll: true,
};

const mapStateToProps = (state) => {
  return {
    isLoaded: getIsLoaded(state),
    isTabletView: getIsTabletView(state),
  };
};
export default connect(mapStateToProps)(SectionBody);
