import React from 'react'
import styled, { css } from 'styled-components';
import device from '../device'
import Label from '../label'

const horizontalCss = css`
  display: flex;
  flex-direction: row;
  align-items: start;
  margin: 0 0 16px 0;

  .field-label {
    line-height: 32px;
    margin: 0;
    width: 110px;
  }
`
const verticalCss = css`
  display: flex;
  flex-direction: column;
  align-items: start;
  margin: 0 0 16px 0;

  .field-label {
    line-height: unset;
    margin: 0 0 4px 0;
    width: auto;
    flex-grow: 1;
  }
`

const Container = styled.div`
  ${props => props.vertical ? verticalCss : horizontalCss }

  @media ${device.tablet} {
    ${verticalCss}
  }
`;

const Body = styled.div`
  flex-grow: 1;
`;

const FieldContainer = React.memo((props) => {
  const {isVertical, className, isRequired, hasError, labelText, children} = props;
  return (
    <Container vertical={isVertical} className={className}>
      <Label isRequired={isRequired} error={hasError} text={labelText} className="field-label"/>
      <Body>{children}</Body>
    </Container>
  );
});

export default FieldContainer