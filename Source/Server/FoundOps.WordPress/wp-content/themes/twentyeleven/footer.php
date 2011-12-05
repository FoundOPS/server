<?php
/**
 * The template for displaying the footer.
 *
 * Contains the closing of the id=main div and all content after
 *
 * @package WordPress
 * @subpackage Twenty_Eleven
 * @since Twenty Eleven 1.0
 */
?>

	</div><!-- #main -->

	<footer id="colophon" role="contentinfo">

			<?php
				/* A sidebar in the footer? Yep. You can can customize
				 * your footer with three columns of widgets.
				 */
				get_sidebar( 'footer' );
			?>
	</footer><!-- #colophon -->
</div><!-- #page -->
<div class="clearfooter"></div>
</div><!-- #container -->
<?php wp_footer(); ?>
<div id="footer">
    <table id="footerList">
    	<tr>
            <td style="color:#ccc; top:2px; padding-right: 5px">Connect&nbsp;&nbsp;&nbsp;</td>
            <td id="facebook"><a href="https://www.facebook.com/pages/FoundOPS/202962066405323">&nbsp;&nbsp;&nbsp;&nbsp;</a></td>
            <td id="linkedin"><a href="http://www.linkedin.com/company/foundops">&nbsp;&nbsp;&nbsp;&nbsp;</a></td>
            <td id="twitter"><a href="http://twitter.com/#!/FoundOPS">&nbsp;&nbsp;&nbsp;&nbsp;</a></td>
        </tr>
    </table>
    <br />
    <div><table id="bottomTable" class="tabs">
            <tr>
                <td></td>
                <td class="bigFooterText"> <a href="<?php echo $GLOBALS["blogLink"];?>/beta">FoundOPS</a></td>
                <td class="bigFooterText"> <a href="<?php echo $GLOBALS["blogLink"];?>/team">About Us</a> </td>
            </tr>
            <tr>
            	<td></td>
                <td><a href="<?php echo $GLOBALS["blogLink"];?>/beta">Beta</a> </td>
                <td> <a href="<?php echo $GLOBALS["blogLink"];?>/team">The Team</a></td>
            </tr>
            <tr>
            	<td></td>
                <td><a href="<?php echo $GLOBALS["blogLink"];?>/product">Features</a></td>
                <td> <a href="<?php echo $GLOBALS["blogLink"];?>/values">Our Values</a> </td>
            </tr>
            <tr>
                <td></td>
                <td></td>
                <td><a href="<?php echo $GLOBALS["blogLink"];?>/jobs">Jobs</a></td>
            </tr>
            <tr>
            	<td rowspan="3" style="line-height:50px;"> <div id="navLogo"><a href="<?php echo $GLOBALS["foundopsLink"];?>">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a></div></td>
                <td class="bigFooterText"><a style="position:relative; top:-10px; left:0px;" href="<?php echo $GLOBALS["blogLink"];?>">Blog</a></td>
                <td><a href="<?php echo $GLOBALS["blogLink"];?>/contact">Contact Us</a></td>
            </tr>
        </table></div>
    <div style="font-size:12px">&copy;2011&nbsp;FOUNDOPS&nbsp;LLC</div><br/>
    <div id="site-generator">
				<?php do_action( 'twentyeleven_credits' ); ?>
				<a href="<?php echo esc_url( __( 'http://wordpress.org/', 'twentyeleven' ) ); ?>" title="<?php esc_attr_e( 'Semantic Personal Publishing Platform', 'twentyeleven' ); ?>" rel="generator"><?php printf( __( 'Proudly powered by %s', 'twentyeleven' ), 'WordPress' ); ?></a>
			</div>
</div>
</body>
</html>