import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { utils, Scrollbar } from "asc-web-components";
import DragAndDrop from "../../DragAndDrop";
import SelectedFrame from "./SelectedFrame";
const { tablet } = utils.device;

const commonStyles = css`
  flex-grow: 1;
  height: 100%;

  .section-wrapper-content {
    flex: 1 0 auto;
    padding: 16px 8px 16px 24px;
    outline: none;

    @media ${tablet} {
      padding: 16px 0 16px 24px;
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
  
  ${props => props.withScroll && `
    margin-left: -24px;
  `}
`;

const StyledDropZoneBody = styled(DragAndDrop)`
  ${commonStyles}

  .drag-and-drop {
    height: 100%;
  }
  
  ${props => props.withScroll && `
    margin-left: -24px;
  `}
`;

const StyledSpacer = styled.div`
  display: none;
  min-height: 64px;

  @media ${tablet} {
    display: ${props => (props.pinned ? "none" : "block")};
  }
`;

class SectionBody extends React.Component {
  constructor(props) {
    super(props);

    this.focusRef = React.createRef();
  }
  
  componentDidMount() {
    if (!this.props.autoFocus) return;
    
    this.focusRef.current.focus();
  }

  render() {
    //console.log("PageLayout SectionBody render");
    const { children, withScroll, autoFocus, pinned, onDrop, uploadFiles, setSelections } = this.props;

    const focusProps = autoFocus ? {
      ref: this.focusRef,
      tabIndex: 1
    } : {};

    const renderBody = () => {
      return(
        withScroll ? (
          <Scrollbar stype="mediumBlack">
            <div className="section-wrapper">
              <div className="section-wrapper-content" {...focusProps}>
                {children}
                <StyledSpacer pinned={pinned} />
              </div>
            </div>
          </Scrollbar>
        ) : (
          <div className="section-wrapper">
            {children}
            <StyledSpacer pinned={pinned} />
          </div>
        ))
    };

    return uploadFiles ? (
      <SelectedFrame setSelections={setSelections}>
        <StyledDropZoneBody
          isDropZone
          onDrop={onDrop}
          withScroll={withScroll}
        >
          {renderBody()}
        </StyledDropZoneBody>
      </SelectedFrame>
    ) : (
      <StyledSectionBody withScroll={withScroll}>
        {renderBody()}
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
    PropTypes.any
  ])
};

SectionBody.defaultProps = {
  withScroll: true,
  autoFocus: false,
  pinned: false,
  uploadFiles: false
};

export default SectionBody;
