import React, { useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import _ from "lodash";
import { PageLayout } from "asc-web-components";
import fakeUsers from './fakseUsers';
import {ArticleHeaderContent, ArticleBodyContent} from './Article';
import {SectionHeaderContent, SectionBodyContent} from './Section';

let selection = [];

const Home = () => {
  const [isHeaderVisible, toggleHeaderVisible] = useState(false);
  const [isHeaderIndeterminate, toggleHeaderIndeterminate] = useState(false);
  const [isHeaderChecked, toggleHeaderChecked] = useState(false);

  let id = -1;

  const onBodySelect = (checked, data) => {
    //toggleChecked(checked);
    id = _.result(
      _.find(selection, function(obj) {
        return obj.id === data.id;
      }),
      "id"
    );
    if (checked && !id) {
      selection.push(data);
    } else if (id) {
      selection = _.filter(selection, function(obj) {
        return obj.id !== id;
      });
    }

    let headerVisible = selection.length > 0;
    let headerIndeterminate = headerVisible && selection.length < fakeUsers.length;
    let headerChecked = !headerIndeterminate;
    
    console.log(`onBodySelect 
        row.checked=${checked} 
        headerVisible=${headerVisible} 
        headerIndeterminate=${headerIndeterminate} 
        headerChecked=${headerChecked}`, 
        data,
        selection);

    if(isHeaderVisible !== headerVisible)
      toggleHeaderVisible(headerVisible);

    if(isHeaderIndeterminate !== headerIndeterminate)
      toggleHeaderIndeterminate(headerIndeterminate);

    if(isHeaderChecked !== headerChecked)
      toggleHeaderChecked(headerChecked);
  };

  return (
    <PageLayout
      articleHeaderContent={ArticleHeaderContent}
      articleBodyContent={ArticleBodyContent}
      sectionHeaderContent={
        <SectionHeaderContent
          isHeaderVisible={isHeaderVisible}
          isHeaderIndeterminate={isHeaderIndeterminate}
          isHeaderChecked={isHeaderChecked}
          onCheck={checked => {
            console.log("SectionHeaderContent onCheck", checked)
          }}
        />
      }
      sectionBodyContent={
        <SectionBodyContent
          users={fakeUsers}
          // isHeaderChecked={isHeaderChecked}
          onSelect={onBodySelect}
        />
      }
    />
  );
};

Home.propTypes = {
  modules: PropTypes.array.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    modules: state.auth.modules,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(withRouter(Home));