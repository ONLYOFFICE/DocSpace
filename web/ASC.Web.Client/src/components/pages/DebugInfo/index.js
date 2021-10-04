import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import ReactMarkdown from "react-markdown";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
import Scrollbar from "@appserver/components/scrollbar";
import axios from "axios";

const DebugInfoDialog = (props) => {
  const { visible, onClose, user } = props;
  const [md, setMd] = useState();

  useEffect(() => {
    async function loadMD() {
      try {
        const response = await axios.get("/debuginfo.md");
        console.log(response, response.data);
        setMd(response.data);
      } catch (e) {
        console.error(e);
        setMd(`Debug info load failed (${e.message})`);
      }
    }

    loadMD();
  }, []);

  return (
    <ModalDialog visible={visible} onClose={onClose} contentHeight="500px">
      <ModalDialog.Header>Debug Info</ModalDialog.Header>
      <ModalDialog.Body>
        {/* <Text>{`# Build version: ${BUILD_VERSION}`}</Text> */}
        <Text>{`# Version: ${VERSION}`}</Text>
        <Text>{`# Build date: ${BUILD_AT}`}</Text>
        {user && (
          <Text>{`# Current User: ${user?.displayName} (id:${user?.id})`}</Text>
        )}
        <Text>{`# User Agent: ${navigator.userAgent}`}</Text>
        <hr />
        <Box overflowProp="auto" heightProp="300px">
          <Scrollbar stype="mediumBlack">
            {md && <ReactMarkdown children={md} />}
          </Scrollbar>
        </Box>
      </ModalDialog.Body>
    </ModalDialog>
  );
};

DebugInfoDialog.propTypes = {
  visible: PropTypes.bool,
  onClose: PropTypes.func,
  personal: PropTypes.bool,
  buildVersionInfo: PropTypes.object,
};

export default inject(({ auth }) => {
  const { user } = auth.userStore;

  return {
    user,
  };
})(observer(DebugInfoDialog));
