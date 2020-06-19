import styled from 'styled-components';

const StyledWizardForm = styled.div`
  width: 976px;
  margin: 0 auto;

  @media(max-width: 1080px) {
      width: 90%;
  }

  .header-base {
    font: normal 18px 'Open Sans', sans-serif;
    margin-bottom: 12px;
  }

  #continue-button {
    margin-top: 20px;
    font: normal 15px 'Open Sans', sans-serif;
  }
`;

export default StyledWizardForm;