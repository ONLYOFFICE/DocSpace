import styled from 'styled-components'

const StyledPasswordBox = styled.div`
  width: 30%;

  .wizard-pass {
    width: 95%;
    margin: 8px 0;
  }

  .label-pass {
    font-size: 12px;
  }

  .label-pass::after {
    content: "*";
    color: red;
  }

  @media(max-width: 768px) {
    width: 50%;

    .wizard-pass {
      width: 95%;
      margin: 8px 0;
    }
  }

  @media(max-width: 600px) {
    width: 100%;

    .wizard-pass {
      width: 98%;
      margin: 8px 0;
    }
  }
`;

export default StyledPasswordBox;