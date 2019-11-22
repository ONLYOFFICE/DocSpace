import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { tablet } from "../../utils/device";
import Label from "../label";
import HelpButton from "../help-button";

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
    min-width: 110px;
    display: inline-flex;
  }
  ${props => (props.vertical ? verticalCss : horizontalCss)}

  @media ${tablet} {
    ${verticalCss}
  }
`;
class FieldContainer extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const {
      isVertical,
      className,
      isRequired,
      hasError,
      labelText,
      children,
      tooltipContent,
      place,
      HelpButtonHeaderContent
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
            <HelpButton
              tooltipContent={tooltipContent}
              place={place}
              HelpButtonHeaderContent={HelpButtonHeaderContent}
            />
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
  place: PropTypes.string,
  HelpButtonHeaderContent: PropTypes.string
};

FieldContainer.defaultProps ={
  place: "bottom"
}

export default FieldContainer;
