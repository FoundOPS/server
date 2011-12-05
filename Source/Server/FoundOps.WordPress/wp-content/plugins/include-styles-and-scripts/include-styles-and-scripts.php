<?php
/*
Plugin Name: Include Styles and Scripts
Plugin URI: http://webdlabs.com/projects/include-styles-and-scripts/
Description: Drop your .js and .css files in the plugin directory and this plugin takes care of the rest, with zero configuration.
Version: 0.1
License: GPL
Author: Akshay Raje
Author URI: http://webdlabs.com
*/

add_action('init', 'inc_styles_scripts_init');

function inc_styles_scripts_init(){
    $inc_styles_scripts_plugin_dir = WP_PLUGIN_DIR.'/'.str_replace(basename( __FILE__),"",plugin_basename(__FILE__));
    $inc_styles_scripts_plugin_url = WP_PLUGIN_URL.'/'.str_replace(basename( __FILE__),"",plugin_basename(__FILE__));

    foreach (glob($inc_styles_scripts_plugin_dir.'*') as $inc_style_script) {
        $inc_id = str_replace($inc_styles_scripts_plugin_dir, '', $inc_style_script);
        if (pathinfo($inc_id, PATHINFO_EXTENSION) == 'css') {
            wp_register_style('inc-style-'.$inc_id, $inc_styles_scripts_plugin_url.$inc_id);
            wp_enqueue_style('inc-style-'.$inc_id);
        } elseif (pathinfo($inc_id, PATHINFO_EXTENSION) == 'js') {
            wp_register_script('inc-script-'.$inc_id, $inc_styles_scripts_plugin_url.$inc_id);
            wp_enqueue_script('inc-script-'.$inc_id);
        }
    }

}

?>