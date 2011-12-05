<?php
/**
 * The Template for displaying all single posts.
 *
 * @package WordPress
 * @subpackage Twenty_Eleven
 * @since Twenty Eleven 1.0
 */

get_header(); ?>

		<div id="primary">
			<div id="content" role="main" style="position:relative; left:140px;">

				<?php while ( have_posts() ) : the_post(); ?>

					<nav id="nav-single" style="position:relative; top:0px; left:100px;">
						<h3 class="assistive-text"><?php _e( 'twentyeleven' ); ?></h3>
						<span class="nav-previous"><?php previous_post_link( '%link', __( '<span class="meta-nav">&larr;</span> Previous', 'twentyeleven' ) ); ?></span>
						<span class="nav-next"><?php next_post_link( '%link', __( 'Next <span class="meta-nav">&rarr;</span>', 'twentyeleven' ) ); ?></span>
					</nav><!-- #nav-single -->

					<?php get_template_part( 'content', 'single' ); ?>
					
					<!-- tweet button -->
					<script src="http://platform.twitter.com/widgets.js" type="text/javascript"></script>
					<table style="margin-left:140px;">
					<tr><td width="130px"><a href="http://twitter.com/share" class="twitter-share-button"
						 data-url="<?php the_permalink(); ?>"
						 data-text="<?php echo "Check out this @FoundOPS blog post, "; the_title(); echo ", by "; the_author(); echo " at "; ?>"
						 data-count="horizontal">Tweet</a></td>
					
					<!-- +1 button -->
					<td><g:plusone size="medium" href="<?php the_permalink(); ?>" width="60"></g:plusone>
					<script type="text/javascript">
					  (function() {
						var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
						po.src = 'https://apis.google.com/js/plusone.js';
						var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
					  })();
					</script></td>
					
					<!-- like button -->
					<td width="110px"><div id="fb-root"></div>
					<script>(function(d, s, id) {
					  var js, fjs = d.getElementsByTagName(s)[0];
					  if (d.getElementById(id)) {return;}
					  js = d.createElement(s); js.id = id;
					  js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
					  fjs.parentNode.insertBefore(js, fjs);
					}(document, 'script', 'facebook-jssdk'));</script>
					<div class="fb-like" data-href="https://www.facebook.com/FoundOPS" data-send="false" data-layout="button_count" data-width="100" style="margin-top:-2px;"></div>
					
					<!-- share button -->
					<td><div style="position:relative; top:2px;"><script src="http://platform.linkedin.com/in.js" type="text/javascript"></script>
					<script type="IN/Share" data-url="http://www.linkedin.com/company/2130457" data-counter="right"></script></div></td>
					</td></tr></table>

					<?php comments_template( '', true ); ?>

				<?php endwhile; // end of the loop. ?>

			</div><!-- #content -->
		</div><!-- #primary -->
<?php get_sidebar(); ?>
<?php get_footer(); ?>