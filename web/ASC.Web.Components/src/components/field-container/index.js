import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { tablet } from "../../utils/device";
import Label from "../label";
import IconButton from "../icon-button";
import Tooltip from "../tooltip";
import { handleAnyClick } from "../../utils/event";
import ReactTooltip from "react-tooltip";

const horizontalCss = css`
  display: flex;
  flex-direction: row;
  align-items: start;
  margin: 0 0 16px 0;

  .field-label {
    line-height: 32px;
    margin: 0;
    position: relative;
  }
  .field-body {
    flex-grow: 1;
  }
  .icon-button {
    position: relative;
    line-height: 24px;
    margin: 2px 0 0 4px;
  }
`;
const verticalCss = css`
  display: flex;
  flex-direction: column;
  align-items: start;
  margin: 0 0 16px 0;

  .field-label {
    line-height: unset;
    margin: 0 0 4px 0;
  }
  .field-body {
    width: 100%;
  }
  .icon-button {
    position: relative;
    line-height: unset;
    margin: -4px 0 0 4px;
  }
`;

const Container = styled.div`
  .field-label-icon {
    width: 110px;
    min-width: 110px;
    display: flex;
  }
  ${props => (props.vertical ? verticalCss : horizontalCss)}

  @media ${tablet} {
    ${verticalCss}
  }
`;
class FieldContainer extends React.Component {
  constructor(props) {
    super(props);

    this.state = { isOpen: false };
    this.ref = React.createRef();
  }

  afterShow = () => {
    handleAnyClick(true, this.handleClick);
  };

  afterHide = () => {
    if (this.state.isOpen) {
      this.setState({ isOpen: false });
    }
  };

  handleClick = e => {
    if (this.state.isOpen && !this.ref.current.contains(e.target)) {
      ReactTooltip.hide();
      this.setState({ isOpen: false });
      handleAnyClick(false, this.handleClick);
    } else if (!this.state.isOpen) {
      this.setState({ isOpen: !this.state.isOpen });
    }
  };

  render() {
    const {
      isVertical,
      className,
      isRequired,
      hasError,
      labelText,
      children,
      tooltipContent,
      tooltipOffsetRight,
      tooltipMaxWidth,
      tooltipId
    } = this.props;

    return (
      <Container vertical={isVertical} className={className}>
        <div className="field-label-icon">
          <Label
            isRequired={isRequired}
            error={hasError}
            text={labelText}
            truncate={true}
            className="field-label"
          />
          {tooltipContent && (
            <div ref={this.ref}>
              <IconButton
                data-tip=""
                data-for={tooltipId}
                data-event="click focus"
                className="icon-button"
                isClickable={true}
                iconName="QuestionIcon"
                size={13}
              />
              <Tooltip
                id={tooltipId}
                effect="solid"
                place="top"
                offsetRight={tooltipOffsetRight}
                maxWidth={tooltipMaxWidth}
                afterShow={this.afterShow}
                afterHide={this.afterHide}
                globalEventOff={true}
              >
                {tooltipContent}
              </Tooltip>
            </div>
          )}
        </div>
        <div className="field-body">{children}</div>
      </Container>
    );
  }
}

FieldContainer.displayName = "FieldContainer";

FieldContainer.propTypes = {
  isVertical: PropTypes.bool,
  className: PropTypes.string,
  isRequired: PropTypes.bool,
  hasError: PropTypes.bool,
  labelText: PropTypes.string,
  icon: PropTypes.string,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  tooltipOffsetRight: PropTypes.number,
  tooltipMaxWidth: PropTypes.number,
  tooltipId: PropTypes.string
};

export default FieldContainer;
