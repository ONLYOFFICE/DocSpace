import styled from "styled-components";

const StyledDataReassignmentLoader = styled.div`
  display: flex;
  flex-direction: column;
  gap: 24px;

  .user {
    display: flex;
    align-items: center;
    gap: 16px;
  }

  .name {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }

  .avatar {
    width: 80px;
    height: 80px;
  }

  .new-owner_header {
    display: flex;
    flex-direction: column;
    gap: 4px;
    padding-bottom: 12px;
  }

  .new-owner_add {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .description {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
`;

export default StyledDataReassignmentLoader;
