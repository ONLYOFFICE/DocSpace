import React from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import toastr from "@appserver/components/toast/toastr";
import Chart from "@appserver/components/chart";
import { tablet, mobile } from "@appserver/components/utils/device";
import { showLoader, hideLoader } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";

class Statistics extends React.Component {
  constructor(props) {
    super(props);
    const { t } = props;
    document.title = `${t("ManagementCategoryStatistic")}`;

    /*     this.state = {
      isLoading: false,
    }; */
  }

  getRandData = (items) => {
    return items.map(
      () => Math.floor(Math.random() * (Math.floor(100) - 0)) + 0
    );
  };

  render() {
    const { t, i18n } = this.props;

    const labels = Array.from({ length: 7 }, (v, i) => "Jun " + i);

    const data = {
      labels: labels,
      datasets: [
        {
          label: "Visits",
          data: this.getRandData(labels),
        },
      ],
    };

    return (
      <Box>
        <Text>{t("VisitsGraph")}</Text>
        <Text>{t("ActiveUsers", { current: 10, total: 50 })}</Text>
        <Box>
          <Chart height="200px" data={data} />
        </Box>
      </Box>
    );
  }
}

Statistics.propTypes = {
  t: PropTypes.func.isRequired,
  i18n: PropTypes.object.isRequired,
};

export default inject(() => {
  return {};
})(withTranslation("Settings")(observer(Statistics)));
