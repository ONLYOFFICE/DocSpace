import styled from 'styled-components';

const StyledWelcomeBox = styled.div`
  .wizard-title {
    font-size: 28px;
    margin-bottom: 15px;
    font: normal 28px 'Open Sans', sans-serif;
  }

  .wizard-desc{
    position: relative;
    display: inline-block;
    font-size: 12px;
    margin-left: 30px;
  }

  .wizard-desc::before {
    display: block;
    position: absolute;
    content: url("images/lock.png");
    left: -30px;
    top: -6px;
  }
`;

export default StyledWelcomeBox;