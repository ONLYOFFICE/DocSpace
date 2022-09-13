import styled from "styled-components";
import { tablet } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  border: ${(props) => props.theme.profile.themePreview.border};
  border-radius: 12px;
  height: 284px;
  width: 318px;
  overflow: hidden;

  @media ${tablet} {
    width: 100%;
  }

  .card-header {
    padding: 12px 20px;
    border-bottom: ${(props) => props.theme.profile.themePreview.border};
  }
`;
