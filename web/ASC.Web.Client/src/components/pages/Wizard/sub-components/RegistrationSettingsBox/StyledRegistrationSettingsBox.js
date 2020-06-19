import styled from 'styled-components';

const StyledRegistrationSettingsBox = styled.div`
  width: 100%;
  padding-left: 7px;
  padding-top: 3px;

  .settings-label {
    font: normal 14px 'Open Sans', sans-serif;
    display: inline-block;
    margin: 2px 14px 11px 0;
  }

  #input-email {
    display: inline-block;
    height: 24px;
    width: 45%;

    @media(max-width: 985px) {
      width: 70%;
    }

    @media(max-width: 768px) {
      width: 95%;
    }

    @media(max-width: 600px) {
      width: 85%;
    }
  }

  .domain-name {
    display: inline-block;
    font: bold 14px 'Open Sans', sans-serif;
    margin-right: 5px;
    @media(max-width: 600px) {
      display: block;
    }
  }

  .question-icon {
    display: inline-block;
    vertical-align: middle;
  }

  @media(max-width: 600px) {
    padding-left: 0;
  }
`;

export default StyledRegistrationSettingsBox;