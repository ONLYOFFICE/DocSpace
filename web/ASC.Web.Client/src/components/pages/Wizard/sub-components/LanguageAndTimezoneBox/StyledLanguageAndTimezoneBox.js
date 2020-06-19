import styled from 'styled-components';

const StyledLanguageAndTimezoneBox = styled.div`
  padding-left: 7px;

  @media(max-width: 600px) {
    padding-left: 0;
  }

  .timezone {
    margin-top: 12px;
  }

  .timezone-lang-label { 
    font: bold 12px 'Open Sans', sans-serif;
    margin-bottom: 5px;
  }

  .lang {
    display: inline-block;
    margin-right: 5px;
    
  }

  .question-icon {
    display: inline-block;
    vertical-align: middle;
  }

  .comboBox {
    width: 350px;
  }
`;

export default StyledLanguageAndTimezoneBox;