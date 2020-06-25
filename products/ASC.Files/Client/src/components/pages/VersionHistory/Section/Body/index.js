import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { Row, RowContainer, Link, Text, Box } from "asc-web-components";

//import i18n from '../../i18n';

const VersionBadge = (props) => (
  <svg width="55" height="18" viewBox="0 0 55 18" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path fill-rule="evenodd" clip-rule="evenodd" d="M0 1C0 0.447716 0.447715 0 1 0L53.9994 0C54.6787 0 55.1603 0.662806 54.9505 1.3089L52.5529 8.6911C52.4877 8.89187 52.4877 9.10813 52.5529 9.3089L54.9505 16.6911C55.1603 17.3372 54.6787 18 53.9994 18H0.999999C0.447714 18 0 17.5523 0 17V1Z" fill="#A3A9AE"/>
  </svg>);

class SectionBodyContent extends React.PureComponent {
  renderRow = info => {
    const title = `${new Date(info.created).toLocaleString(this.props.culture)} ${info.createdBy.displayName}`;
    return (
        <Row key={info.id}>
            <Box marginProp="0 8px" displayProp="flex">
              <VersionBadge />
              <Text style={{position: "absolute", left: "16px"}} color="#FFFFFF" isBold fontSize="12px">Ver.{info.version}</Text>
            </Box>
            <Link fontWeight={600} fontSize="14px" title={title}>{title}</Link>
        </Row>
    );
  };
  render() {
    const { versions } = this.props;
    console.log("VersionHistory SectionBodyContent render()", versions);
    return <RowContainer useReactWindow={false}>{versions.map(this.renderRow)}</RowContainer>;
  }
}

export default withRouter(withTranslation()(SectionBodyContent));
