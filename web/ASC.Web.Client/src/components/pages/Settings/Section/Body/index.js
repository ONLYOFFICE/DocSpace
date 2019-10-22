import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";

class SectionBodyContent extends React.PureComponent {

  render() {
    return (
      <div>
        Test
      </div>
    );
  };
};

function mapStateToProps(state) {
  return {

  };
}

export default connect(mapStateToProps)(withRouter(SectionBodyContent));
