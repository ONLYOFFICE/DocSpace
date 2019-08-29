import React from 'react'
import styled from 'styled-components';
import device from '../device'
import Label from '../label'

const Container = styled.div`
  display: flex;
  flex-direction: row;
  margin: 0 0 16px 0;

  .field-label {
    line-height: 32px;
    margin: 0;
    width: 110px;
  }

  .field-input {
    width: 320px;
  }

  .radio-group {
    line-height: 32px;
    display: flex;

    label:not(:first-child) {
        margin-left: 33px;
    }
  }

  @media ${device.tablet} {
    flex-direction: column;
    align-items: start;

    .field-label {
      line-height: unset;
      margin: 0 0 4px 0;
      width: auto;
      flex-grow: 1;
    }
  }
`;

const Body = styled.div`
  flex-grow: 1;
`;

const FieldContainer = React.memo((props) => {
  const {isRequired, hasError, labelText, className, children} = props;
  return (
    <Container className={className}>
      <Label isRequired={isRequired} error={hasError} text={labelText} className="field-label"/>
      <Body>{children}</Body>
    </Container>
  );
});

export default FieldContainer