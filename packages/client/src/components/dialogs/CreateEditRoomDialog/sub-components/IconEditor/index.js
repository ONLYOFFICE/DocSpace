import React from "react";
import styled from "styled-components";

const StyledIconEditorWrapper = styled.div`
  .use_modal-buttons_wrapper {
    display: none;
  }
`;

const StyledIconEditor = styled(AvatarEditor)`
  margin: 0;
`;

import AvatarEditor from "@docspace/components/avatar-editor";

const IconEditor = () => {
  return (
    <StyledIconEditorWrapper>
      <StyledIconEditor
        className="icon-editor"
        useModalDialog={false}
      ></StyledIconEditor>
    </StyledIconEditorWrapper>
  );
};

export default IconEditor;
