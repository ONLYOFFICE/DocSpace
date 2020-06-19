import styled from 'styled-components';

const StyledHeaderBox = styled.header`
  .wizard-header {
    height: 48px; 
    background-color: #0f4071;
    width: 100%;
    position: relative;
    margin-bottom: 74px;
  
    .wizard-logo {
      display: block;
      width: 142px;
      height: 23px;
      cursor: pointer;
      margin:  -12px 0 0;
      margin-left: 24px;
      position:  absolute;
      top: 51%;
    }
  }

  .wizard-header::before {
    content: "";
    background: url("images/clouds_fond.png");
    height: 48px;
    position: absolute;
    top: 48px;
    width: 100%;
  }
`;

export default StyledHeaderBox;